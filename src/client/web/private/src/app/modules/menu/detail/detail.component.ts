import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { ConfirmDialogComponent } from 'src/app/components/confirm-dialog/confirm-dialog.component';
import { DetailModel } from 'src/app/models/detail-model';
import { MenuModel } from 'src/app/models/menu-model';
import { GlobalizationDictionaryPipe } from 'src/app/pipes/globalization-dictionary.pipe';
import { GlobalizationMessagesPipe } from 'src/app/pipes/globalization-messages.pipe';
import { MainService } from 'src/app/services/main.service';
import { MenuService } from 'src/app/services/menu.service';

@Component({
  selector: 'app-detail',
  templateUrl: './detail.component.html',
  styleUrls: ['./detail.component.css']
})
export class DetailComponent implements OnInit {

  model = new DetailModel<MenuModel>();
  loading = true;
  id: string;

  constructor(
    private route: ActivatedRoute,
    private serviceUser:  MenuService,
    private serviceMain: MainService,
    public globalizationDictionaryPipe: GlobalizationDictionaryPipe,
    public globalizationMessagesPipe: GlobalizationMessagesPipe,
    private router: Router,
    public dialog: MatDialog,
  ) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      this.id = params.get('id');
      this.fillModel();
    });
    
  }


  fillModel(): void {
    this.serviceUser.detail(this.id).subscribe(
      res => {
        if (res.status === 200) {
          this.model = res.body as DetailModel<MenuModel>;
        } else {
          this.serviceMain.openErrorSnackBar('Kod: 01. ' + res.statusText);
        }
        this.loading = false;
      },
      err => {
        if (err.status === 400) {
          if (err.error != null) {
            this.model = err.error;
            const errors = Object.keys(err.error).map((t) => {
              return err.error[t];
            });
            this.serviceMain.openErrorSnackBar('Kod: 01. ' + errors.toString());
          } else {
            this.serviceMain.openErrorSnackBar('Kod: 01. ' + err.error);
          }
          this.loading = false;
        }
        setTimeout(() => {
          this.router.navigate(['/Menu/List']);
        }, 1000);
      }
    );


  }

  back(): void {
    this.router.navigate(['/Menu/List']);
  }

  update(): void {
    console.log(this.id);
    this.router.navigate(['/Menu/Update', this.id]);
  }

  showConfirm(): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: this.globalizationMessagesPipe.transform('QuestionAreYouSure'),
        message: this.globalizationMessagesPipe.transform('QuestionAreYouSureDelete')
      }
    });

    dialogRef.afterClosed().subscribe(dialogResult => {
      if (dialogResult === true) {
        this.delete();
      }
    });
  }

  delete(): void {
    this.serviceUser.delete(this.id).subscribe(
      res => {
        if (res.status === 200) {
          this.serviceMain.openSuccessSnackBar(this.globalizationMessagesPipe.transform('InfoDeletionOperationSuccessful'));
        } else {
          this.serviceMain.openErrorSnackBar('Kod: 01. ' + res.statusText);
        }
        setTimeout(() => {
          this.router.navigate(['/Menu/List']);
        }, 3000);
      },
      err => {
        if (err.status === 400) {
          if (err.error != null) {
            this.loading = false;
            this.model = err.error;
            const errors = Object.keys(err.error).map((t) => {
              return err.error[t];
            });
            this.serviceMain.openErrorSnackBar('Kod: 01. ' + errors.toString());
          } else {
            this.loading = false;
            this.serviceMain.openErrorSnackBar('Kod: 01. ' + err.error);
          }
        }
        setTimeout(() => {
          this.router.navigate(['/Menu/List']);
        }, 3000);
      }
    );
  }
}
