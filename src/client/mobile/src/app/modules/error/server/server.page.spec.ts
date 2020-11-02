import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { IonicModule } from '@ionic/angular';

import { ServerPage } from './server.page';

describe('ServerPage', () => {
  let component: ServerPage;
  let fixture: ComponentFixture<ServerPage>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ServerPage ],
      imports: [IonicModule.forRoot()]
    }).compileComponents();

    fixture = TestBed.createComponent(ServerPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  }));

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
