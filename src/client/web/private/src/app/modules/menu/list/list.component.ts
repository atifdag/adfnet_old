import { SelectionModel } from '@angular/cdk/collections';
import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';
import { MatDatepickerInputEvent } from '@angular/material/datepicker';

import { MatDialog } from '@angular/material/dialog';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { Router } from '@angular/router';
import { ConfirmDialogComponent } from 'src/app/components/confirm-dialog/confirm-dialog.component';
import { FilterModelWithMultiParent } from 'src/app/models/filter-model-with-multi-parent';
import { ListModel } from 'src/app/models/list-model';
import { MenuModel } from 'src/app/models/menu-model';
import { GlobalizationDictionaryPipe } from 'src/app/pipes/globalization-dictionary.pipe';
import { GlobalizationMessagesPipe } from 'src/app/pipes/globalization-messages.pipe';
import { AppSettingsService } from 'src/app/services/app-settings.service';
import { MainService } from 'src/app/services/main.service';
import { MenuService } from 'src/app/services/menu.service';

@Component({
  selector: 'app-list',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.css']
})
export class ListComponent implements OnInit {

  displayedColumns: string[] = [ 'id', 'code', 'name', 'address','parentMenu', 'actions'];
  listModel = new ListModel<MenuModel>();
  dataSource:MatTableDataSource<MenuModel>;
  pageEvent: PageEvent;
  selection = new SelectionModel<MenuModel>(true, []);
  selectedPageNumber = 1;
  filterModel = new FilterModelWithMultiParent();
  menuForm: FormGroup;
  yearRange: string;

  statusOptions: any[];
  pageSizes: number[] = [];
  selectedPageSize = this.appSettingsService.selectedPageSize.key;
  rowsPerPageOptions = this.appSettingsService.rowsPerPageOptions;

  loading = true;

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  constructor(
    private appSettingsService: AppSettingsService,
    private serviceMenu:MenuService,
    private serviceMain:MainService,
    public dialog: MatDialog,
    public globalizationDictionaryPipe: GlobalizationDictionaryPipe,
    public globalizationMessagesPipe: GlobalizationMessagesPipe,
    private router: Router
    ) {
      const startDate = new Date();
      const endDate = new Date();
      const today = new Date();
      const year = today.getFullYear();
      const invalidDate = new Date();
      invalidDate.setDate(today.getDate() - 1);
      this.yearRange = (year - 10).toString() + ':' + year.toString();
      startDate.setFullYear(today.getFullYear() - 2);
      this.statusOptions = this.appSettingsService.statusOptions;

      
      this.appSettingsService.pageSizes.forEach(x => {
        this.pageSizes.push(x.key);
      });
   
  
      this.filterModel.startDate = startDate;
      this.filterModel.endDate = endDate;
      this.filterModel.status = -1;

    
      this.filterModel.pageNumber = this.selectedPageNumber;
      this.filterModel.pageSize = this.selectedPageSize;
      this.listModel.items = [];


     }

  ngOnInit(): void {


    this.menuForm = new FormGroup({

      searched: new FormControl(this.filterModel.searched),      
       selectedStatus: new FormControl(this.appSettingsService.selectedStatus.key),
       startDate: new FormControl(this.filterModel.startDate),
       endDate: new FormControl(this.filterModel.endDate)
 


    });
    this.list();
  }


  list()
  {
     this.serviceMenu.list().subscribe(res=>{  

      if (res.status === 200)
      { 
          this.listModel = res.body as ListModel<MenuModel>;
          console.log(this.listModel);
          this.dataSource = new MatTableDataSource(this.listModel.items);
         this.dataSource.paginator = this.paginator;

         this.paginator.length = this.listModel.items.length;

          console.log(this.listModel.items.length);

          this.dataSource.sort = this.sort;
        
      }
      else {
         console.log("hataya düştü");

      }

     });
  }

  filter(): void {

    this.serviceMenu.filter(this.filterModel).subscribe(
      res => {
        if (res.status === 200) {
          this.listModel = res.body as ListModel<MenuModel>;
          this.dataSource = new MatTableDataSource(this.listModel.items);
          this.pageSizes.push(this.dataSource.data.length);
          this.paginator.length = this.listModel.items.length;
          this.dataSource.paginator = this.paginator;
          this.dataSource.sort = this.sort;

        } else {
          this.serviceMain.openErrorSnackBar('Kod: 11. ' + res.statusText);

        }
        this.loading = false;
      },
      err => {
        if (err.status === 400) {
          if (err.error != null) {
            this.listModel.message = 'Hata oluştu!';
            this.listModel.hasError = true;
            this.serviceMain.openErrorSnackBar('Kod: 12. ' + this.listModel.message);

          } else {
            this.listModel.message = err.error;
            this.listModel.hasError = true;
            this.serviceMain.openErrorSnackBar('Kod: 13 ' + this.listModel.message);
          }
          this.loading = false;
          setTimeout(() => {
            this.router.navigate(['/User/List']);
          }, 1000);
        }
      }
    );
  }

  changeForm(): void {


    this.loading = true;
    this.filterModel.pageNumber = this.selectedPageNumber;
    this.filterModel.pageSize = this.selectedPageSize;

   
    this.filter();
  }

  handlePage(event): void {

    this.selectedPageNumber = event.pageIndex + 1;
    this.filterModel.pageNumber = this.selectedPageNumber;
    this.selectedPageSize = event.pageSize;
    this.filterModel.pageSize = this.selectedPageSize;
    this.changeForm();
  }
  showConfirm(id: string): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: this.globalizationMessagesPipe.transform('QuestionAreYouSure'),
        message: this.globalizationMessagesPipe.transform('QuestionAreYouSureDelete')
      }
    });

    dialogRef.afterClosed().subscribe(dialogResult => {
      if (dialogResult === true) {
        this.delete(id);
      }
    });
  }

  startDateEvent(event: MatDatepickerInputEvent<Date>): void {
    this.menuForm.get('startDate').setValue(event.value);
    this.changeForm();
  }

  endDateEvent(event: MatDatepickerInputEvent<Date>): void {
    this.menuForm.get('endDate').setValue(event.value);
    this.changeForm();
  }
  delete(id: string): void {
    this.loading= true;
    this.serviceMenu.delete(id).subscribe(
      res => {
        if (res.status === 200) {
          this.serviceMain.openSuccessSnackBar(this.globalizationMessagesPipe.transform('InfoDeletionOperationSuccessful'));
        } else {
          this.serviceMain.openErrorSnackBar('Kod: 01. ' + res.statusText);
        }

        this.list();
      },
      err => {
        if (err.status === 400) {
          if (err.error != null) {
            const errors = Object.keys(err.error).map((t) => {
              return err.error[t];
            });
            this.serviceMain.openErrorSnackBar('Kod: 01. ' + errors.toString());
          } else {

            this.serviceMain.openErrorSnackBar('Kod: 01. ' + err.error);
          }
        }
        this.list();
      }
    );
  }


}
