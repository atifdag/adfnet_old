import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { IonicModule } from '@ionic/angular';



@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    FormsModule,
    IonicModule,
  ],
  exports:[
    CommonModule,
    FormsModule,
    IonicModule,
  ]
})
export class SharedModule { }
