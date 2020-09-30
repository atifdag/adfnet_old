import { CdkTextareaAutosize } from '@angular/cdk/text-field/autosize';
import { Component, NgZone, OnInit, ViewChild } from '@angular/core';
import {take} from 'rxjs/operators';

@Component({
  selector: 'app-add',
  templateUrl: './add.component.html',
  styleUrls: ['./add.component.css']
})
export class AddComponent implements OnInit {

  @ViewChild('autosize') autosize: CdkTextareaAutosize;

  constructor(private ngZone: NgZone) { }

  ngOnInit(): void {
  }

  triggerResize(): void {
    this.ngZone.onStable.pipe(take(1)).subscribe(() => this.autosize.resizeToFitContent(true));
  }
}
