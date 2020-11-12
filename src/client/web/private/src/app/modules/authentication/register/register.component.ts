import { TitleCasePipe } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { RegisterModel } from 'src/app/models/register-model';
import { GlobalizationDictionaryPipe } from 'src/app/pipes/globalization-dictionary.pipe';
import { GlobalizationMessagesPipe } from 'src/app/pipes/globalization-messages.pipe';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { IdentityService } from 'src/app/services/identity.service';
import { MainService } from 'src/app/services/main.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  userForm: FormGroup;
  submitted: boolean;
  loading: boolean;
  model: RegisterModel;
  constructor(
    private titlecasePipe: TitleCasePipe,
    private router: Router,
    private fb: FormBuilder,
    private serviceAuthentication: AuthenticationService,
    private serviceMain: MainService,
    public globalizationDictionaryPipe: GlobalizationDictionaryPipe,
    public globalizationMessagesPipe: GlobalizationMessagesPipe,
    private identityService: IdentityService,

  ) { }

  ngOnInit(): void {
    this.userForm = this.fb.group({
      firstName: new FormControl('', [Validators.required, Validators.minLength(2)]),
      lastName: new FormControl('', [Validators.required, Validators.minLength(2)]),
      username: new FormControl('', [Validators.required, Validators.minLength(8)]),
      email: new FormControl('', [Validators.required, Validators.email]),
      password: new FormControl('', [Validators.required, Validators.minLength(8)]),
      confirmPassword: [],
      isApproved: new FormControl(true)
    }, {
      validator: this.checkConfirmPassword
    });
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


  checkConfirmPassword(group: FormGroup): boolean {
    const password = group.controls.password.value;
    const confirmPassword = group.controls.confirmPassword.value;
    return password === confirmPassword ? null : true;
  }

  globalizationMessagesByParameter(key: string, parameter: string): string {
    return this.globalizationMessagesPipe.transform(key + ',' + parameter);
  }
  globalizationMessages(key: string): string {
    return this.globalizationMessagesPipe.transform(key);
  }


  globalizationMessagesByParameter2(key: string, parameter1: string, parameter2: string): string {
    return this.globalizationMessagesPipe.transform(key + ',' + parameter1 + ',' + parameter2);
  }
  submit(): void {
    this.submitted = true;
    if (this.userForm.invalid) {
      return;
    }
    this.loading = true;
    this.model = new RegisterModel();


    this.model.firstName = this.userForm.controls.firstName.value;
    this.model.lastName = this.userForm.controls.lastName.value;
    this.model.username = this.userForm.controls.username.value;
    this.model.email = this.userForm.controls.email.value;
    this.model.password = this.userForm.controls.password.value;
    this.model.confirmPassword = this.userForm.controls.confirmPassword.value;


    this.serviceAuthentication.register(this.model).subscribe(
      res => {
        if (res.status === 200) {
          this.identityService.set(res.body);
          this.serviceMain.openSuccessSnackBar('Kaydetme işlemi başarıyla tamamlandı. E-posta gelen kutunuzu kontrol ediniz. '
            + ' ' + this.globalizationDictionaryPipe.transform('RedirectionTitle'));
          setTimeout(() => {
            this.router.navigate(['/Authentication/Login']);
          }, 3000);
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
              return err.error[t] + ' ';
            });
            this.serviceMain.openErrorSnackBar('Kod: 02. ' + errors.toString());
          } else {
            this.loading = false;
            this.serviceMain.openErrorSnackBar('Kod: 99. ' + err.error);
          }
        }
      }
    );

  }

}
