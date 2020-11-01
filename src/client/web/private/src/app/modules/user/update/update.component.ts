import { DatePipe, TitleCasePipe } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { DetailModel } from 'src/app/models/detail-model';
import { UpdateModel } from 'src/app/models/update-model';
import { UserModel } from 'src/app/models/user-model';
import { GlobalizationDictionaryPipe } from 'src/app/pipes/globalization-dictionary.pipe';
import { GlobalizationMessagesPipe } from 'src/app/pipes/globalization-messages.pipe';
import { MainService } from 'src/app/services/main.service';
import { UserService } from 'src/app/services/user.service';

@Component({
  selector: 'app-update',
  templateUrl: './update.component.html',
  styleUrls: ['./update.component.css']
})
export class UpdateComponent implements OnInit {
  model = new UpdateModel<UserModel>();
  userForm: FormGroup;
  submitted: boolean;
  id: string;
  loading = true;
  constructor(
    private titlecasePipe: TitleCasePipe,
    private router: Router,
    private datePipe: DatePipe,
    public globalizationDictionaryPipe: GlobalizationDictionaryPipe,
    public globalizationMessagesPipe: GlobalizationMessagesPipe,
    private serviceUser: UserService,
    private serviceMain: MainService,
    private route: ActivatedRoute,
  ) {
    this.model.item = new UserModel();


  }



  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      this.id = params.get('id');

      this.fillModel();
    });

    this.userForm = new FormGroup({
      id: new FormControl({ disabled: true }, Validators.required),
      username: new FormControl('', [Validators.required, Validators.minLength(5)]),
      email: new FormControl('', [Validators.required, Validators.email]),
      firstName: new FormControl('', [Validators.required, Validators.minLength(2)]),
      lastName: new FormControl('', [Validators.required, Validators.minLength(2)]),
      language: new FormControl('', [Validators.required]),
      isApproved: new FormControl()
    });

    this.userForm.get('language').setValue(this.model.item.language.id);
  }

  fillModel(): void {
    this.serviceUser.beforeUpdate(this.id).subscribe(
      res => {
        if (res.status === 200) {
          this.model = res.body as UpdateModel<UserModel>;
          this.userForm.get('id').setValue(this.model.item.id);
          this.userForm.get('firstName').setValue(this.model.item.firstName);
          this.userForm.get('lastName').setValue(this.model.item.lastName);
          this.userForm.get('username').setValue(this.model.item.username);
          this.userForm.get('email').setValue(this.model.item.email);
          this.userForm.get('language').setValue(this.model.item.language.id);
          this.userForm.get('isApproved').setValue(this.model.item.isApproved);

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
        // setTimeout(() => {
        //   this.router.navigate(['/User/List']);
        // }, 1000);
      }
    );


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


  back(): void {
    this.router.navigate(['/User/List']);
  }


  submit(): void {
    this.submitted = true;
    if (this.userForm.invalid) {
      return;
    }
    this.loading = true;
    if (this.model.item == null) {
      this.model.item = new UserModel();
    }
    this.model.item.username = this.userForm.controls.username.value;
    this.model.item.email = this.userForm.controls.email.value;
    this.model.item.firstName = this.userForm.controls.firstName.value;
    this.model.item.lastName = this.userForm.controls.lastName.value;
    this.model.item.language.id = this.userForm.controls.language.value;
    this.model.item.isApproved = this.userForm.controls.isApproved.value;
    this.serviceUser.update(this.model).subscribe(
      res => {
        if (res.status === 200) {
          this.serviceMain.openSuccessSnackBar(this.globalizationMessagesPipe.transform('InfoSaveOperationSuccessful')
            + ' ' + this.globalizationDictionaryPipe.transform('RedirectionTitle'));
          setTimeout(() => {

            this.router.navigate(['/User/List']);
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

}
