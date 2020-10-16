import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { ConfirmDialogComponent } from 'src/app/components/confirm-dialog/confirm-dialog.component';
import { MyProfileModel } from 'src/app/models/my-profile-model';
import { GlobalizationDictionaryPipe } from 'src/app/pipes/globalization-dictionary.pipe';
import { GlobalizationMessagesPipe } from 'src/app/pipes/globalization-messages.pipe';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { MainService } from 'src/app/services/main.service';
import { UserService } from 'src/app/services/user.service';
import { SignOutOption } from 'src/app/value-objects/sign-out-option.enum';

@Component({
  selector: 'app-my-profile',
  templateUrl: './my-profile.component.html',
  styleUrls: ['./my-profile.component.css']
})
export class MyProfileComponent implements OnInit {
  profileModel = new MyProfileModel();
  constructor(
    private router: Router,
    public globalizationDictionaryPipe: GlobalizationDictionaryPipe,
    private globalizationMessagesPipe: GlobalizationMessagesPipe,
    private serviceUser: UserService,
    private serviceAuthentication: AuthenticationService,
    public dialog: MatDialog,
    private serviceMain: MainService,
  ) { }

  ngOnInit(): void {
    if (sessionStorage.getItem('myProfile')) {
      const jsonObj: MyProfileModel = JSON.parse(
        sessionStorage.getItem('myProfile')

      );
      this.profileModel = jsonObj as MyProfileModel;
    } else {
      this.serviceUser.myProfile().subscribe(
        res => {
          if (res.status === 200) {
            this.profileModel = res.body as MyProfileModel;
            sessionStorage.setItem('myProfile', JSON.stringify(this.profileModel));
          } else {

            this.serviceMain.openErrorSnackBar('Kod: 01. ' + res.statusText);
          }
        },
        err => {
          if (err.status === 400) {
            if (err.error != null) {
              this.serviceMain.openErrorSnackBar('Kod: 02. ' + err.error.message);
            } else {
              this.serviceMain.openErrorSnackBar('Kod: 03. ' + err);
            }
          }
        }

      );
    }
  }

  showConfirm(): void {

    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: this.globalizationMessagesPipe.transform('QuestionAreYouSure'),
        message: this.globalizationMessagesPipe.transform('QuestionAreYouSureLogout')
      }
    });

    // listen to response
    dialogRef.afterClosed().subscribe(dialogResult => {
      if (dialogResult === true){
        this.signOut();
      }
    });
  }

  signOut(): void {
    this.serviceAuthentication.signOut(SignOutOption.ValidLogout).subscribe();
    this.serviceMain.openSuccessSnackBar(this.globalizationMessagesPipe.transform('InfoLogoutOperationSuccessful')
      + ' ' + this.globalizationDictionaryPipe.transform('RedirectionTitle'));
    setTimeout(() => {
      this.router.navigate(['/Home']);
    }, 1000);
  }

}
