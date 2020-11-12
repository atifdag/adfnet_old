import { UserModel } from './../../../models/user-model';
import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { SelectionModel } from '@angular/cdk/collections';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { FilterModelWithMultiParent } from 'src/app/models/filter-model-with-multi-parent';
import { ListModel } from 'src/app/models/list-model';
import { Router } from '@angular/router';
import { GlobalizationDictionaryPipe } from 'src/app/pipes/globalization-dictionary.pipe';
import { GlobalizationMessagesPipe } from 'src/app/pipes/globalization-messages.pipe';
import { AppSettingsService } from 'src/app/services/app-settings.service';
import { UserService } from 'src/app/services/user.service';
import { MainService } from 'src/app/services/main.service';
import { FormGroup, FormControl } from '@angular/forms';
import { IdCodeNameSelected } from 'src/app/value-objects/id-code-name-selected';
import { MatDatepickerInputEvent } from '@angular/material/datepicker';
import { RoleService } from 'src/app/services/role.service';
import { IdName } from 'src/app/value-objects/id-name';
import { ConfirmDialogComponent } from 'src/app/components/confirm-dialog/confirm-dialog.component';
import { MatDialog } from '@angular/material/dialog';

@Component({
  selector: 'app-list',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.css']
})
export class ListComponent implements OnInit {

  displayedColumns: string[] = ['select', 'id', 'username', 'firstName', 'lastName', 'roles', 'actions'];
  dataSource: MatTableDataSource<UserModel>;
  selection = new SelectionModel<UserModel>(true, []);
  roles: IdName[] = [];
  pageEvent: PageEvent;
  selectedPageNumber = 1;
  statusOptions: any[];
  pageSizes: number[] = [];
  selectedPageSize = this.appSettingsService.selectedPageSize.key;
  rowsPerPageOptions = this.appSettingsService.rowsPerPageOptions;
  listModel = new ListModel<UserModel>();
  filterModel = new FilterModelWithMultiParent();
  yearRange: string;
  loading = true;
  userForm: FormGroup;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  constructor(
    private appSettingsService: AppSettingsService,
    private serviceUser: UserService,
    public globalizationDictionaryPipe: GlobalizationDictionaryPipe,
    public globalizationMessagesPipe: GlobalizationMessagesPipe,
    private serviceRole: RoleService,
    private serviceMain: MainService,
    public dialog: MatDialog,
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
    this.filterModel.pageNumber = this.selectedPageNumber;
    this.filterModel.pageSize = this.selectedPageSize;
    this.listModel.items = [];



  }
  ngOnInit(): void {

    this.userForm = new FormGroup({
      searched: new FormControl(this.filterModel.searched),
      startDate: new FormControl(this.filterModel.startDate),
      endDate: new FormControl(this.filterModel.endDate),
      selectedStatus: new FormControl(this.appSettingsService.selectedStatus.key),
      selectedRoles: new FormControl(''),
    });
    this.getRoles();


  }


  getRoles(): void {
    this.serviceRole.idNameList().subscribe(
      responseRole => {
        if (responseRole.status === 200) {
          const idNameList = responseRole.body as IdName[];
          if (idNameList.length > 0) {
            this.roles = idNameList;
            this.list();
          } else {
            this.serviceMain.openErrorSnackBar('Kod: 22. ' + responseRole.statusText);

          }
        } else {
          this.serviceMain.openErrorSnackBar('Kod: 23. ' + this.globalizationMessagesPipe.transform('DangerParentNotFound'));

        }
      },
      errorRole => {
        this.loading = false;
        if (errorRole.status === 400) {
          this.loading = false;
          this.serviceMain.openErrorSnackBar('Kod: 24. ' + this.globalizationMessagesPipe.transform('DangerParentNotFound'));

        }
        setTimeout(() => {
          this.router.navigate(['/Home']);
        }, 1000);
      }
    );
  }

  list(): void {
    this.serviceUser.list().subscribe(
      res => {
        if (res.status === 200) {
          this.listModel = res.body as ListModel<UserModel>;
          this.dataSource = new MatTableDataSource(this.listModel.items);
          this.paginator.length = this.listModel.items.length;
          this.dataSource.paginator = this.paginator;
          this.dataSource.sort = this.sort;
        } else {
          this.serviceMain.openErrorSnackBar('Kod: 01. ' + res.statusText);

        }
        this.loading = false;
      },
      err => {
        if (err.status === 400) {
          if (err.error != null) {
            this.listModel.message = 'Hata oluştu!';
            this.listModel.hasError = true;
            this.serviceMain.openErrorSnackBar('Kod: 02. ' + this.listModel.message);

          } else {
            this.listModel.message = err.error;
            this.listModel.hasError = true;
            this.serviceMain.openErrorSnackBar('Kod: 03 ' + this.listModel.message);
          }
          this.loading = false;
          setTimeout(() => {
            this.router.navigate(['/User/List']);
          }, 1000);
        }
      }
    );
  }

  getPageSizeOptions(): number[] {
    return [5, 10, 20, this.dataSource.paginator.length];
  }


  filter(): void {

    this.serviceUser.filter(this.filterModel).subscribe(
      res => {
        if (res.status === 200) {
          this.listModel = res.body as ListModel<UserModel>;
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
    this.filterModel.searched = this.userForm.controls.searched.value;
    this.filterModel.startDate = this.userForm.controls.startDate.value;
    this.filterModel.endDate = this.userForm.controls.endDate.value;
    this.filterModel.status = this.userForm.controls.selectedStatus.value;
    this.filterModel.pageNumber = this.selectedPageNumber;
    this.filterModel.pageSize = this.selectedPageSize;

    if (this.userForm.controls.selectedRoles.value !== '') {
      this.filterModel.parents = [];
      this.userForm.controls.selectedRoles.value.forEach((x: string) => {
        const idCodeNameSelected = new IdCodeNameSelected();
        idCodeNameSelected.id = x;
        this.filterModel.parents.push(idCodeNameSelected);
      });
    }



    this.filter();
  }

  handlePage(event): void {

    this.selectedPageNumber = event.pageIndex + 1;
    this.filterModel.pageNumber = this.selectedPageNumber;
    this.selectedPageSize = event.pageSize;
    this.filterModel.pageSize = this.selectedPageSize;
    this.changeForm();
  }

  startDateEvent(event: MatDatepickerInputEvent<Date>): void {
    this.userForm.get('startDate').setValue(event.value);
    this.changeForm();
  }

  endDateEvent(event: MatDatepickerInputEvent<Date>): void {
    this.userForm.get('endDate').setValue(event.value);
    this.changeForm();
  }
  isAllSelected(): boolean {
    const numSelected = this.selection.selected.length;
    const numRows = this.listModel.items.length;
    return numSelected === numRows;
  }

  masterToggle(): void {
    this.isAllSelected() ?
      this.selection.clear() :
      this.dataSource.data.forEach(row => this.selection.select(row));
  }

  checkboxLabel(row?: UserModel): string {
    if (!row) {
      return `${this.isAllSelected() ? 'select' : 'deselect'} all`;
    }
    return `${this.selection.isSelected(row) ? 'deselect' : 'select'} row ${row.id + 1}`;
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

  delete(id: string): void {
    this.loading= true;
    this.serviceUser.delete(id).subscribe(
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
