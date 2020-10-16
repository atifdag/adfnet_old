import { WaitingSnackbarComponent } from './../components/waiting-snackbar/waiting-snackbar.component';
import { Injectable } from '@angular/core';
import { AppSettingsService } from './app-settings.service';
import { Observable } from 'rxjs/internal/Observable';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { MatSnackBar } from '@angular/material/snack-bar';
import { GlobalizationDictionaryPipe } from '../pipes/globalization-dictionary.pipe';

@Injectable({
  providedIn: 'root'
})
export class MainService {

  constructor(
    private snackBar: MatSnackBar,
    private appSettingsService: AppSettingsService,

    public globalizationDictionaryPipe: GlobalizationDictionaryPipe,
    private httpClient: HttpClient
  ) { }

  test(): Observable<HttpResponse<any>> {
    return this.httpClient.get(this.appSettingsService.apiUrl + '/Home/Index', { observe: 'response' });
  }

  openSuccessSnackBar(message: string): void {
    this.snackBar.open(this.globalizationDictionaryPipe.transform('Success') + '! ' + message, 'X', {
      duration: 1000,
      horizontalPosition: 'right',
      verticalPosition: 'bottom',
      panelClass: ['success-snackbar']
    });
  }
  openErrorSnackBar(message: string): void {
    this.snackBar.open(this.globalizationDictionaryPipe.transform('Error') + '! ' + message, 'X', {
      duration: 3000,
      horizontalPosition: 'center',
      verticalPosition: 'bottom',
      panelClass: ['error-snackbar']
    });
  }

  convertToSeoLiteral(str: string): string {

    str = str.replace(/^\s+|\s+$/g, ''); // trim
    str = str.toLowerCase();

    const from = 'ÇçĞğIıİiÖöŞş';
    const to = 'ccggiiiiooss';
    for (let i = 0, l = from.length; i < l; i++) {
      str = str.replace(new RegExp(from.charAt(i), 'g'), to.charAt(i));
    }

    str = str.replace(/[^a-z0-9 -]/g, '') // remove invalid chars
      .replace(/\s+/g, '-') // collapse whitespace and replace by -
      .replace(/-+/g, '-'); // collapse dashes

    return str;
  }


  upperCase(str: string): string {
    str = str.replace('Ç', 'Ç');
    str = str.replace('Ğ', 'Ğ');
    str = str.replace('I', 'I');
    str = str.replace('İ', 'İ');
    str = str.replace('Ö', 'Ö');
    str = str.replace('Ş', 'Ş');
    str = str.replace('Ü', 'Ü');
    str = str.replace('ç', 'Ç');
    str = str.replace('ğ', 'Ğ');
    str = str.replace('ı', 'I');
    str = str.replace('i', 'İ');
    str = str.replace('ö', 'Ö');
    str = str.replace('ş', 'Ş');
    str = str.replace('ü', 'Ü');
    str = str.toLocaleUpperCase();
    return str;
  }

  globalizationKeys(): Observable<HttpResponse<any>> {
    return this.httpClient.get(
      this.appSettingsService.apiUrl + '/Home/GlobalizationKeys',
      { observe: 'response' }
    );
  }
}
