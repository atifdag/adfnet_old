import { Injectable } from '@angular/core';
import { AppSettingsService } from './app-settings.service';

@Injectable({
  providedIn: 'root'
})
export class IdentityService {


  constructor(
    private appSettingsService: AppSettingsService,

  ) { }

  set(value: string): void {
    this.remove();
    sessionStorage.setItem(this.appSettingsService.tokenKey, value);
    // localStorage.setItem(this.appSettings.tokenKey, value);
  }

  get(): string {
    return sessionStorage.getItem(this.appSettingsService.tokenKey);
    // return localStorage.getItem(this.appSettings.tokenKey);
  }

  remove(): void {
    sessionStorage.removeItem(this.appSettingsService.tokenKey);
    sessionStorage.removeItem('myProfile');
    // localStorage.removeItem(this.appSettings.tokenKey);
  }

  isLogged(): boolean {
    if (this.get() != null) {
      if (this.get().length > 0) {
        return true;
      } else {
        return false;
      }
    } else {
      return false;
    }
  }
}
