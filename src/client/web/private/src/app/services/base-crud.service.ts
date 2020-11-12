import { UpdateModel } from '../models/update-model';
import { AddModel } from '../models/add-model';
import { FilterModel } from '../models/filter-model';
import { Inject, Injectable } from '@angular/core';
import { AppSettingsService } from './app-settings.service';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs/internal/Observable';

@Injectable({
  providedIn: 'root'
})
export abstract class BaseCrudService<T> {

  constructor(
    protected appSettingsService: AppSettingsService,
    protected httpClient: HttpClient,
    @Inject(String) private endPoint: any
  ) { }

  list(): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl + '/' + this.endPoint + '/List',
      { observe: 'response' }
    );
  }

  filter(model: FilterModel): Observable<HttpResponse<any>> {
    return this.httpClient.post(
      this.appSettingsService.apiUrl + '/' + this.endPoint + '/Filter',
      model,
      { observe: 'response' });
  }

  detail(id: string): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl + '/' + this.endPoint + '/Detail/' + id,
      { observe: 'response' }
    );
  }

  beforeAdd(): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl + '/' + this.endPoint + '/Add',
      { observe: 'response' }
    );
  }

  add(model: AddModel<T>): Observable<HttpResponse<any>> {
    return this.httpClient.post(
      this.appSettingsService.apiUrl + '/' + this.endPoint + '/Add',
      model,
      { observe: 'response' });
  }

  beforeUpdate(id: string): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl + '/' + this.endPoint + '/Update/' + id,
      { observe: 'response' }
    );
  }

  update(model: UpdateModel<T>): Observable<HttpResponse<any>> {
    return this.httpClient.put(
      this.appSettingsService.apiUrl + '/' + this.endPoint + '/Update',
      model,
      { observe: 'response' });
  }

  delete(id: string): Observable<HttpResponse<any>> {
    return this.httpClient.delete(
      this.appSettingsService.apiUrl + '/' + this.endPoint + '/Delete/' + id,
      { observe: 'response' }
    );
  }
}
