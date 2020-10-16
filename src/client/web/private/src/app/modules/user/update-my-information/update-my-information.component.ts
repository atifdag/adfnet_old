import { DatePipe, TitleCasePipe } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MyProfileModel } from 'src/app/models/my-profile-model';
import { UpdateMyInformationModel } from 'src/app/models/update-my-information-model';
import { GlobalizationDictionaryPipe } from 'src/app/pipes/globalization-dictionary.pipe';
import { GlobalizationMessagesPipe } from 'src/app/pipes/globalization-messages.pipe';
import { MainService } from 'src/app/services/main.service';
import { UserService } from 'src/app/services/user.service';

@Component({
  selector: 'app-update-my-information',
  templateUrl: './update-my-information.component.html',
  styleUrls: ['./update-my-information.component.css']
})
export class UpdateMyInformationComponent implements OnInit {
  userForm: FormGroup;
  submitted: boolean;
  loading = false;
  model: UpdateMyInformationModel;
  profileModel = new MyProfileModel();
  constructor(
    private titlecasePipe: TitleCasePipe,
    private serviceUser: UserService,
    private serviceMain: MainService,
    private router: Router,
    private datePipe: DatePipe,
    public globalizationDictionaryPipe: GlobalizationDictionaryPipe,
    public globalizationMessagesPipe: GlobalizationMessagesPipe,
  ) { }

  ngOnInit(): void {
    const jsonObj: MyProfileModel = JSON.parse(
      sessionStorage.getItem('myProfile')
    );
    this.profileModel = jsonObj as MyProfileModel;
    this.userForm = new FormGroup({
      id: new FormControl({ value: this.profileModel.userModel.id, disabled: true }, Validators.required),
      username: new FormControl(this.profileModel.userModel.username, [Validators.required, Validators.minLength(5)]),
      email: new FormControl(this.profileModel.userModel.email, [Validators.required, Validators.email]),
      firstName: new FormControl(this.profileModel.userModel.firstName, [Validators.required, Validators.minLength(2)]),
      lastName: new FormControl(this.profileModel.userModel.lastName, [Validators.required, Validators.minLength(2)]),
      language: new FormControl(this.profileModel.userModel.language.name, [Validators.required]),
      creator: new FormControl(this.profileModel.userModel.creator.name),
      creationTime: new FormControl(
        this.datePipe.transform(this.profileModel.userModel.creationTime, 'dd/MM/yyyy HH:mm:ss')
      ),
      lastModifier: new FormControl(this.profileModel.userModel.lastModifier.name),
      lastModificationTime: new FormControl(
        this.datePipe.transform(this.profileModel.userModel.lastModificationTime, 'dd/MM/yyyy HH:mm:ss')
      ),
    });

    this.userForm.get('language').setValue(this.profileModel.userModel.language.id);
  }


  globalizationMessages(key: string): string {
    return this.globalizationMessagesPipe.transform(key);
  }

  globalizationMessagesByParameter(key: string, parameter: string): string {
    return this.globalizationMessagesPipe.transform(key + ',' + parameter);
  }

  globalizationMessagesByParameter2(key: string, parameter1: string, parameter2: string): string {
    return this.globalizationMessagesPipe.transform(key + ',' + parameter1 + ',' + parameter2);
  }

  onBlurFirstName(): void {
    const firstName = this.userForm.controls.firstName.value;
    this.userForm.get('firstName').setValue(this.titlecasePipe.transform(firstName));
  }

  onBlurLastName(): void {
    const firstName = this.userForm.controls.firstName.value;
    const lastName = this.userForm.controls.lastName.value;
    this.userForm.get('lastName').setValue(this.serviceMain.upperCase(lastName));
    this.userForm.get('username').setValue(this.serviceMain.convertToSeoLiteral(firstName)
      + '.'
      + this.serviceMain.convertToSeoLiteral(lastName));
  }
  submit(): void {
    this.submitted = true;
    if (this.userForm.invalid) {
      return;
    }
    this.loading = true;
    this.model = new UpdateMyInformationModel();
    this.model.username = this.userForm.controls.username.value;
    this.model.email = this.userForm.controls.email.value;
    this.model.firstName = this.userForm.controls.firstName.value;
    this.model.lastName = this.userForm.controls.lastName.value;
    this.model.language.id = this.userForm.controls.language.value;
    sessionStorage.removeItem('myProfile');
    this.serviceUser.updateMyInformation(this.model).subscribe(
      res => {
        if (res.status === 200) {
          this.serviceMain.openSuccessSnackBar(this.globalizationMessagesPipe.transform('InfoSaveOperationSuccessful')
            + ' ' + this.globalizationDictionaryPipe.transform('RedirectionTitle'));
          setTimeout(() => {

            this.router.navigate(['/User/MyProfile']);
          }, 1000);
        } else {
          this.loading = false;
          this.serviceMain.openErrorSnackBar('Kod: 01. ' + res.statusText);
        }
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
      }
    );

  }

  backClick(): void {
    this.router.navigate(['/User/MyProfile']);
  }
}
