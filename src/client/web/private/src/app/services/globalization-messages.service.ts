import { Injectable } from '@angular/core';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs/internal/Observable';
import { AppSettingsService } from './app-settings.service';

@Injectable({
  providedIn: 'root'
})
export class GlobalizationMessagesService {

  constructor(
    private appSettingsService: AppSettingsService,
    private httpClient: HttpClient
  ) { }

  get(key: string): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl
      + '/GlobalizationMessages/Get?key='
      + key,
      { observe: 'response' }
    );
  }

  getByParameter(key: string, parameter: string): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl
      + '/GlobalizationMessages/GetByParameter?key='
      + key + '&parameter='
      + parameter,
      { observe: 'response' }
    );
  }
  getByParameter2(key: string, parameter1: string, parameter2: string): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl
      + '/GlobalizationMessages/GetByParameter2?key='
      + key + '&parameter1='
      + parameter1 + '&parameter2='
      + parameter2,
      { observe: 'response' }
    );
  }


}
