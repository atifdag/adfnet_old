import { UserService } from 'src/app/services/user.service';
import { MediaMatcher } from '@angular/cdk/layout';
import { ChangeDetectorRef, Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MyProfileModel } from 'src/app/models/my-profile-model';
import { MainService } from 'src/app/services/main.service';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { SignOutOption } from 'src/app/value-objects/sign-out-option.enum';
import { GlobalizationDictionaryPipe } from 'src/app/pipes/globalization-dictionary.pipe';
import { GlobalizationMessagesPipe } from 'src/app/pipes/globalization-messages.pipe';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmDialogComponent } from 'src/app/components/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-private-layout',
  templateUrl: './private-layout.component.html',
  styleUrls: ['./private-layout.component.css']
})
export class PrivateLayoutComponent implements OnInit, OnDestroy {
  profileModel = new MyProfileModel();
  mobileQuery: MediaQueryList;
  private mobileQueryListener: () => void;
  loading = true;
  constructor(
    private serviceMain: MainService,
    private serviceUser: UserService,
    changeDetectorRef: ChangeDetectorRef,
    private router: Router,
    private serviceAuthentication: AuthenticationService,
    public globalizationDictionaryPipe: GlobalizationDictionaryPipe,
    private globalizationMessagesPipe: GlobalizationMessagesPipe,
    public dialog: MatDialog,
    media: MediaMatcher
  ) {
    this.mobileQuery = media.matchMedia('(max-width: 600px)');
    this.mobileQueryListener = () => changeDetectorRef.detectChanges();
    this.mobileQuery.addEventListener('change', this.mobileQueryListener);
  }

  ngOnInit(): void {

    if (sessionStorage.getItem('myProfile')) {
      const jsonObj: MyProfileModel = JSON.parse(
        sessionStorage.getItem('myProfile')
      );
      this.profileModel = jsonObj as MyProfileModel;
      this.loading = false;
    } else {
      this.serviceUser.myProfile().subscribe(
        res => {
          this.loading = false;
          if (res.status === 200) {
            this.profileModel = res.body as MyProfileModel;
            sessionStorage.setItem('myProfile', JSON.stringify(this.profileModel));
          } else {
            this.serviceMain.openErrorSnackBar('Kod: 01. ' + res.statusText);

          }
        },
        err => {
          this.loading = false;
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
  ngOnDestroy(): void {
    this.mobileQuery.removeEventListener('change', this.mobileQueryListener);
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
