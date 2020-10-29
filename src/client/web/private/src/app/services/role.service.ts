import { Injectable } from '@angular/core';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { AppSettingsService } from './app-settings.service';
import { RoleModel } from '../models/role-model';
import { Observable } from 'rxjs/internal/Observable';
import { BaseCrudService } from './base-crud.service';

@Injectable({
  providedIn: 'root'
})
export class RoleService extends BaseCrudService<RoleModel>{

  constructor(
    protected appSettingsService: AppSettingsService,
    protected httpClient: HttpClient,
  ) {
    super(appSettingsService, httpClient, 'Role');
  }

  idNameList(): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl + '/Role/IdNameList',
      { observe: 'response' }
    );
  }


}
