import { Component, OnInit } from '@angular/core';
import { AppSettingsService } from 'src/app/services/app-settings.service';
import { MainService } from 'src/app/services/main.service';

@Component({
  selector: 'app-index',
  templateUrl: './index.component.html',
  styleUrls: ['./index.component.css']
})
export class IndexComponent implements OnInit {

  constructor( private appSettingsService: AppSettingsService, private serviceMain: MainService,) { }

  ngOnInit(): void {
    this.serviceMain.test().subscribe(
      res => {
          if (res.status === 200) {

          } else {
          }
      },
      err => {

      }
  );
  }

}
