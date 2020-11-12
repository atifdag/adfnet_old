import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { UpdatePasswordModel } from 'src/app/models/update-password-model';
import { GlobalizationDictionaryPipe } from 'src/app/pipes/globalization-dictionary.pipe';
import { GlobalizationMessagesPipe } from 'src/app/pipes/globalization-messages.pipe';
import { MainService } from 'src/app/services/main.service';
import { UserService } from 'src/app/services/user.service';

@Component({
  selector: 'app-update-my-password',
  templateUrl: './update-my-password.component.html',
  styleUrls: ['./update-my-password.component.css']
})
export class UpdateMyPasswordComponent implements OnInit {
  userForm: FormGroup;
  submitted: boolean;
  loading = true;
  model: UpdatePasswordModel;
  constructor(
    private fb: FormBuilder,
    private serviceUser: UserService,
    private router: Router,
    public globalizationDictionaryPipe: GlobalizationDictionaryPipe,
    private globalizationMessagesPipe: GlobalizationMessagesPipe,
    private serviceMain: MainService,
  ) { }

  ngOnInit(): void {
    this.userForm = this.fb.group({
      oldPassword: new FormControl('', Validators.required),
      password: new FormControl('', Validators.required),
      confirmPassword: []
    }, {
      validator: this.checkConfirmPassword
    });
    this.loading = false;
  }


  checkConfirmPassword(group: FormGroup): boolean {
    const password = group.controls.password.value;
    const confirmPassword = group.controls.confirmPassword.value;
    return password === confirmPassword ? null : true;
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
    this.model = new UpdatePasswordModel();
    this.model.oldPassword = this.userForm.controls.oldPassword.value;
    this.model.password = this.userForm.controls.password.value;
    this.model.confirmPassword = this.userForm.controls.confirmPassword.value;
    this.serviceUser.updateMyPassword(this.model).subscribe(
      res => {
        if (res.status === 200) {
          this.serviceMain.openSuccessSnackBar('Kaydetme işlemi başarıyla tamamlandı. E-posta gelen kutunuzu kontrol ediniz. '
            + ' ' + this.globalizationDictionaryPipe.transform('RedirectionTitle'));
          setTimeout(() => {
            sessionStorage.removeItem('myProfile');
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
            this.serviceMain.openErrorSnackBar('Kod: 02. ' + errors.toString());

          } else {
            this.loading = false;
            this.serviceMain.openErrorSnackBar('Kod: 02. ' + err.error);

          }
        }
      }
    );

  }

  backClick(): void {
    this.router.navigate(['/User/MyProfile']);
  }
}
