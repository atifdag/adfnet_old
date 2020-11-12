import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UpdateMyPasswordComponent } from './update-my-password.component';

describe('UpdateMyPasswordComponent', () => {
  let component: UpdateMyPasswordComponent;
  let fixture: ComponentFixture<UpdateMyPasswordComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ UpdateMyPasswordComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(UpdateMyPasswordComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
