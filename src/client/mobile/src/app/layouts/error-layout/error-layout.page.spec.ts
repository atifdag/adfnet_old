import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { IonicModule } from '@ionic/angular';

import { ErrorLayoutPage } from './error-layout.page';

describe('ErrorLayoutPage', () => {
  let component: ErrorLayoutPage;
  let fixture: ComponentFixture<ErrorLayoutPage>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ErrorLayoutPage ],
      imports: [IonicModule.forRoot()]
    }).compileComponents();

    fixture = TestBed.createComponent(ErrorLayoutPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  }));

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
