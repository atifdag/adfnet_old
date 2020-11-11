import { Router } from '@angular/router';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-index',
  templateUrl: './index.component.html',
  styleUrls: ['./index.component.scss'],
})
export class IndexComponent implements OnInit {

  constructor(
    private router: Router,
  ) { }

  ngOnInit() {}

  login(): void {
    this.router.navigate(['/Authentication/Login']);
  }

}
