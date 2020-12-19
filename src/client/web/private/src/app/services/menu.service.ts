import { Injectable } from '@angular/core';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { AppSettingsService } from './app-settings.service';
import { FilterModelWithMultiParent } from '../models/filter-model-with-multi-parent';
import { Observable } from 'rxjs/internal/Observable';
import { UpdatePasswordModel } from '../models/update-password-model';
import { UpdateMyInformationModel } from '../models/update-my-information-model';
import { BaseCrudService } from './base-crud.service';
import { MenuModel } from '../models/menu-model';
import { FormGroup } from '@angular/forms';

@Injectable({
  providedIn: 'root'
})
export class MenuService extends BaseCrudService<MenuModel>{

  constructor(
    protected appSettingsService: AppSettingsService,
    protected httpClient: HttpClient,
  ) {
    super(appSettingsService, httpClient, 'Menu');
  }

  filter(model: FilterModelWithMultiParent): Observable<HttpResponse<any>> {
    return this.httpClient.post(
      this.appSettingsService.apiUrl + '/Menu/FilterWithMultiParent',
      model,
      { observe: 'response' });
  }

  myProfile(): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl + '/Menu/MyProfile',
      { observe: 'response' }
    );
  }

  updateMyPassword(model: UpdatePasswordModel): Observable<HttpResponse<any>> {
    return this.httpClient.put(this.appSettingsService.apiUrl + '/Menu/UpdateMyPassword', model, { observe: 'response' });
  }

  updateMyInformation(model: UpdateMyInformationModel): Observable<HttpResponse<any>> {
    return this.httpClient.put(this.appSettingsService.apiUrl + '/Menu/UpdateMyInformation', model, { observe: 'response' });
  }

}
