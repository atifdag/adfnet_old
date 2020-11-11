import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AlertController } from '@ionic/angular';

@Component({
  selector: 'app-tabs',
  templateUrl: './tabs.component.html',
  styleUrls: ['./tabs.component.scss'],
})
export class TabsComponent implements OnInit {

  constructor(
    public alertController: AlertController,
    private router: Router,
  ) { }

  ngOnInit() {}
  async presentAlertConfirm() {
    const alert = await this.alertController.create({
      header: 'Emin misiniz?',
      cssClass: 'custom-alert',
      message: 'Sistemden çıkış yapmak istediğinizden emin misiniz?',
      buttons: [
        {
          text: 'Evet',
          handler: () => {
            this.router.navigate(['/Authentication/Login']);
          }
        },
        {
          text: 'Hayır',
          role: 'cancel',
          cssClass: 'secondary',
        },

      ]
    });

    await alert.present();
  }
}
