import { IdCodeNameSelected } from 'src/app/value-objects/id-code-name-selected';
import { UserService } from 'src/app/services/user.service';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { DetailModel } from 'src/app/models/detail-model';
import { UserModel } from 'src/app/models/user-model';
import { MainService } from 'src/app/services/main.service';
import { GlobalizationDictionaryPipe } from 'src/app/pipes/globalization-dictionary.pipe';
import { GlobalizationMessagesPipe } from 'src/app/pipes/globalization-messages.pipe';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmDialogComponent } from 'src/app/components/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-detail',
  templateUrl: './detail.component.html',
  styleUrls: ['./detail.component.css']
})
export class DetailComponent implements OnInit {
  loading = true;
  id: string;
  model = new DetailModel<UserModel>();
  constructor(
    private route: ActivatedRoute,
    private serviceUser: UserService,
    private serviceMain: MainService,
    public globalizationDictionaryPipe: GlobalizationDictionaryPipe,
    public globalizationMessagesPipe: GlobalizationMessagesPipe,
    private router: Router,
    public dialog: MatDialog,
  ) {
    this.model.item = new UserModel();

  }


  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      this.id = params.get('id');
      this.fillModel();
    });
  }


  selectedRoles(): IdCodeNameSelected[] {
    return this.model.item.roles.filter(x => x.selected === true);
  }

  fillModel(): void {
    this.serviceUser.detail(this.id).subscribe(
      res => {
        if (res.status === 200) {
          this.model = res.body as DetailModel<UserModel>;

        } else {
          this.loading = false;
          this.serviceMain.openErrorSnackBar('Kod: 01. ' + res.statusText);
        }
      },
      err => {
        this.loading = false;
        if (err.status === 400) {
          if (err.error != null) {
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
          this.router.navigate(['/User/List']);
        }, 1000);
      }
    );


  }

  back(): void {
    this.router.navigate(['/User/List']);
  }

  update(): void {
    console.log(this.id);
    this.router.navigate(['/User/Update', { id: this.id }]);
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
          this.router.navigate(['/User/List']);
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
          this.router.navigate(['/User/List']);
        }, 3000);
      }
    );
  }


}
