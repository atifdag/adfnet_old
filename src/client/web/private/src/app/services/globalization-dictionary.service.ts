import { Injectable } from '@angular/core';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs/internal/Observable';
import { AppSettingsService } from './app-settings.service';

@Injectable({
  providedIn: 'root'
})
export class GlobalizationDictionaryService {

  constructor(
    private appSettingsService: AppSettingsService,
    private httpClient: HttpClient
  ) { }

  get(key: string): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl
      + '/GlobalizationDictionary/Get?key=' + key,
      { observe: 'response' }
    );
  }
}
