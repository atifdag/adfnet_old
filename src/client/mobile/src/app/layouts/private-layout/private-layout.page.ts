import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AlertController } from '@ionic/angular';

@Component({
  selector: 'app-private-layout',
  templateUrl: './private-layout.page.html',
  styleUrls: ['./private-layout.page.scss'],
})
export class PrivateLayoutPage implements OnInit {
  public appPages = [
    {
      title: 'Home',
      url: '/home',
      icon: 'home-outline'
    },
    {
      title: 'List',
      url: '/list',
      icon: 'list'
    }
  ];
  constructor(
    private router: Router,
    public alertController: AlertController
  ) {

  }


  ngOnInit() {
  }



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
