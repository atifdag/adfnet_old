import { Injectable } from '@angular/core';
import { AppSettingsService } from './app-settings.service';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { IdentityService } from './identity.service';
import { Observable } from 'rxjs/internal/Observable';
import { LoginModel } from '../models/login-model';
import { SignOutOption } from '../value-objects/sign-out-option.enum';
import { RegisterModel } from '../models/register-model';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {

  constructor(
    private appSettingsService: AppSettingsService,
    private httpClient: HttpClient,
    private svcIdentity: IdentityService,
  ) { }

  login(model: LoginModel): Observable<HttpResponse<any>> {
    model.key = this.appSettingsService.jwtSecurityKey;
    return this.httpClient.post(this.appSettingsService.apiUrl + '/Authentication/Login', model, { observe: 'response' });
  }

  signOut(signOutOption: SignOutOption): Observable<HttpResponse<any>> {
    this.svcIdentity.remove();
    return this.httpClient.get(this.appSettingsService.apiUrl + '/Authentication/SignOut?signOutOption=' + signOutOption, { observe: 'response' });
  }

  forgotPassword(username: string): Observable<HttpResponse<any>> {
    return this.httpClient.get(this.appSettingsService.apiUrl + '/Authentication/ForgotPassword' + '?username=' + username, { observe: 'response' });
  }

  register(model: RegisterModel): Observable<HttpResponse<any>> {
    return this.httpClient.post(this.appSettingsService.apiUrl + '/Authentication/Register', model, { observe: 'response' });
  }
}
