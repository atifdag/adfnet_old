import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { GlobalizationDictionaryPipe } from 'src/app/pipes/globalization-dictionary.pipe';
import { GlobalizationMessagesPipe } from 'src/app/pipes/globalization-messages.pipe';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { MainService } from 'src/app/services/main.service';

@Component({
  selector: 'app-forgot-password',
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.css']
})
export class ForgotPasswordComponent implements OnInit {
  userForm: FormGroup;
  loading: boolean;
  submitted: boolean;
  constructor(
    private fb: FormBuilder,
    private serviceAuthentication: AuthenticationService,
    private serviceMain: MainService,
    private router: Router,
    private globalizationMessagesPipe: GlobalizationMessagesPipe,
    public globalizationDictionaryPipe: GlobalizationDictionaryPipe,
  ) { }

  ngOnInit(): void {
    this.userForm = this.fb.group({
      username: new FormControl('', Validators.required)
    });
  }

  globalizationMessagesByParameter(key: string, parameter: string): string {
    return this.globalizationMessagesPipe.transform(key + ',' + parameter);
  }

  submit(): void {
    this.submitted = true;
    if (this.userForm.invalid) {
      return;
    }
    this.loading = true;
    const username = this.userForm.controls.username.value;

    this.serviceAuthentication.forgotPassword(username).subscribe(
      res => {
        if (res.status === 200) {
          this.serviceMain.openSuccessSnackBar('Şifre başarıyla gönderildi. E-posta gelen kutunuzu kontrol ediniz.'
            + ' ' + this.globalizationDictionaryPipe.transform('RedirectionTitle'));
          setTimeout(() => {
            this.router.navigate(['Authentication/Login']);
          }, 1000);
        } else {
          this.loading = false;
          this.serviceMain.openErrorSnackBar('Kod: 01. ' + res.statusText);
        }
      },
      err => {
        this.loading = false;
        this.serviceMain.openErrorSnackBar('Kod: 99. ' + err.error);
      }
    );

  }

}
