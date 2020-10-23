import { Injectable } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AppSettingsService {

  private previousUrl: string = undefined;
  private currentUrl: string = undefined;

  jwtSecurityKey = 'b29166e3-bc27-42dc-bda5-9ee93c8c5dd7';
  tokenKey = 'customidentitytoken';
  apiUrl = environment.apiUrl;
  calendarTr = {
    firstDayOfWeek: 1,
    dayNames: ['Pazar', 'Pazartesi', 'Salı', 'Çarsamba', 'Perşembe', 'Cuma', 'Cumartesi'],
    dayNamesShort: ['Paz', 'Pts', 'Sal', 'Çar', 'Per', 'Cum', 'Cte'],
    dayNamesMin: ['P', 'P', 'S', 'Ç', 'P', 'C', 'C'],
    monthNames: ['Ocak', 'Şubat', 'Mart', 'Nisan', 'Mayıs', 'Haziran', 'Temmuz', 'Ağustos', 'Eylül', 'Ekim', 'Kasım', 'Aralık'],
    monthNamesShort: ['Oca', 'Şub', 'Mar', 'Nis', 'May', 'Haz', 'Tem', 'Ağu', 'Eyl', 'Eki', 'Kas', 'Ara'],
    today: 'Bugün',
    dateFormat: 'dd/mm/yy',
    clear: 'Temizle'
  };

  selectedStatus = { key: -1, value: '[Tümü]' };
  selectedPageSize = { key: 10, value: '10' };

  statusOptions = [
    { key: -1, value: '[Tümü]' },
    { key: 1, value: 'Onaylı' },
    { key: 0, value: 'Onaysız' },
  ];

  rowsPerPageOptions = [
    5,
    10,
    25,
    50,
    100,
    500,
    { showAll: '[Tümü]' }
  ];

  pageSizes = [
    { key: 1, value: '1' },
    { key: 5, value: '5' },
    { key: 10, value: '10' },
    { key: 25, value: '25' },
    { key: 50, value: '50' },
    { key: 100, value: '100' },
    { key: 500, value: '500' },
  ];

  constructor(
    private router: Router
  ) {
    this.currentUrl = this.router.url;
    router.events.subscribe(event => {
      if (event instanceof NavigationEnd) {
        this.previousUrl = this.currentUrl;
        this.currentUrl = event.url;
      }
    });
  }

  public getPreviousUrl(): string {
    return this.previousUrl;
  }

}
