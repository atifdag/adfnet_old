import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute, Router } from '@angular/router';
import { LoginModel } from 'src/app/models/login-model';
import { GlobalizationDictionaryPipe } from 'src/app/pipes/globalization-dictionary.pipe';
import { GlobalizationMessagesPipe } from 'src/app/pipes/globalization-messages.pipe';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { IdentityService } from 'src/app/services/identity.service';
import { MainService } from 'src/app/services/main.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  userForm: FormGroup;
  submitted: boolean;
  loading: boolean;
  redirectUrl: string;
  model: LoginModel;
  constructor(
    private router: Router,
    private fb: FormBuilder,
    private serviceAuthentication: AuthenticationService,
    private serviceMain: MainService,
    public globalizationDictionaryPipe: GlobalizationDictionaryPipe,
    private globalizationMessagesPipe: GlobalizationMessagesPipe,
    private identityService: IdentityService,

  ) { }


  ngOnInit(): void {
    this.loading = false;
    this.userForm = this.fb.group({
      username: new FormControl('', [Validators.required, Validators.minLength(8)]),
      password: new FormControl('', [Validators.required, Validators.minLength(8)]),
    });
  }

  globalizationMessagesByParameter(key: string, parameter: string): string {
    return this.globalizationMessagesPipe.transform(key + ',' + parameter);
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
    this.model = new LoginModel();
    this.model.username = this.userForm.controls.username.value;
    this.model.password = this.userForm.controls.password.value;

    this.serviceAuthentication.login(this.model).subscribe(
      res => {
        if (res.status === 200) {
          this.identityService.set(res.body);
          this.loading = false;
          let url: any[];
          if (this.redirectUrl != null) {
            if (this.redirectUrl.indexOf(';id=') > 0) {
              const arrUrl = this.redirectUrl.split(';id=');
              url = [arrUrl[0], { id: arrUrl[1] }];
            } else {
              url = [this.redirectUrl];
            }
          } else {
            url = ['/Home/Index'];
          }
          this.router.navigate(url);
        } else if (res.status === 401) {
          this.loading = false;
          this.serviceMain.openErrorSnackBar('Kod: 401. ' + res.body);
        } else if (res.status === 403) {
          this.loading = false;
          this.serviceMain.openErrorSnackBar('Kod: 403. ' + res.body);
        } else {
          this.loading = false;
          this.serviceMain.openErrorSnackBar('Kod: 01. ' + res.body);
        }
      },
      err => {
        this.loading = false;
        this.serviceMain.openErrorSnackBar('Kod: 99. ' + err.error);
      }
    );

  }
}
