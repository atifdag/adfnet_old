import { UserModel } from './../../../models/user-model';
import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { SelectionModel } from '@angular/cdk/collections';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { FilterModelWithMultiParent } from 'src/app/models/filter-model-with-multi-parent';
import { ListModel } from 'src/app/models/list-model';
import { Router } from '@angular/router';
import { GlobalizationDictionaryPipe } from 'src/app/pipes/globalization-dictionary.pipe';
import { GlobalizationMessagesPipe } from 'src/app/pipes/globalization-messages.pipe';
import { AppSettingsService } from 'src/app/services/app-settings.service';
import { UserService } from 'src/app/services/user.service';
import { MainService } from 'src/app/services/main.service';

@Component({
  selector: 'app-list',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.css']
})
export class ListComponent implements AfterViewInit {

  displayedColumns: string[] = ['select', 'id', 'username', 'firstName', 'lastName', 'actions'];
  dataSource: MatTableDataSource<UserModel>;
  selection = new SelectionModel<UserModel>(true, []);
  selectedPageNumber = 1;
  statusOptions: any[];
  pageSizes: any[];
  selectedPageSize = this.appSettingsService.selectedPageSize.key;
  rowsPerPageOptions = this.appSettingsService.rowsPerPageOptions;
  listModel = new ListModel<UserModel>();
  filterModel = new FilterModelWithMultiParent();
  yearRange: string;
  loading = true;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  constructor(
    private appSettingsService: AppSettingsService,
    private serviceUser: UserService,
    public globalizationDictionaryPipe: GlobalizationDictionaryPipe,
    private globalizationMessagesPipe: GlobalizationMessagesPipe,
    private serviceMain: MainService,
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
    this.pageSizes = this.appSettingsService.pageSizes;
    this.filterModel.startDate = startDate;
    this.filterModel.endDate = endDate;
    this.filterModel.pageNumber = this.selectedPageNumber;
    this.filterModel.pageSize = this.selectedPageSize;

    this.list();


  }
  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  list(): void {
    this.serviceUser.list(this.filterModel).subscribe(
      res => {
        if (res.status === 200) {
          this.listModel = res.body as ListModel<UserModel>;
          this.dataSource = new MatTableDataSource(this.listModel.items);
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
  isAllSelected(): boolean {
    const numSelected = this.selection.selected.length;
    const numRows = this.dataSource.data.length;
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

  applyFilter(event: Event): void {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }
}
