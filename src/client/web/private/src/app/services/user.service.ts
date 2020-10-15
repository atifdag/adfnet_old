import { Injectable } from '@angular/core';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { AppSettingsService } from './app-settings.service';
import { UserModel } from '../models/user-model';
import { FilterModelWithMultiParent } from '../models/filter-model-with-multi-parent';
import { Observable } from 'rxjs/internal/Observable';
import { UpdatePasswordModel } from '../models/update-password-model';
import { UpdateMyInformationModel } from '../models/update-my-information-model';
import { BaseCrudService } from './base-crud.service';

@Injectable({
  providedIn: 'root'
})
export class UserService extends BaseCrudService<UserModel>{

  constructor(
    protected appSettingsService: AppSettingsService,
    protected httpClient: HttpClient,
  ) {
    super(appSettingsService, httpClient, 'User');
  }

  filter(model: FilterModelWithMultiParent): Observable<HttpResponse<any>> {
    return this.httpClient.post(
      this.appSettingsService.apiUrl + '/User/FilterWithMultiParent',
      model,
      { observe: 'response' });
  }

  myProfile(): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl + '/User/MyProfile',
      { observe: 'response' }
    );
  }

  updateMyPassword(model: UpdatePasswordModel): Observable<HttpResponse<any>> {
    return this.httpClient.put(this.appSettingsService.apiUrl + '/User/UpdateMyPassword', model, { observe: 'response' });
  }

  updateMyInformation(model: UpdateMyInformationModel): Observable<HttpResponse<any>> {
    return this.httpClient.put(this.appSettingsService.apiUrl + '/User/UpdateMyInformation', model, { observe: 'response' });
  }
}
