import { Component, OnDestroy, OnInit } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { AlertController } from '@ionic/angular';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-private-layout',
  templateUrl: './private-layout.page.html',
  styleUrls: ['./private-layout.page.scss'],
})
export class PrivateLayoutPage implements OnInit, OnDestroy {
  navigationSubscription: Subscription;
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
    this.navigationSubscription = this.router.events.subscribe((e: any) => {
      // If it is a NavigationEnd event re-initalise the component
      if (e instanceof NavigationEnd) {
        this.initialiseInvites();
      }
    });
  }


  ngOnInit() {
  }

  initialiseInvites() {
    // Set default values and re-fetch any data you need.
  }

  ngOnDestroy() {
    if (this.navigationSubscription) {
      this.navigationSubscription.unsubscribe();
    }
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
