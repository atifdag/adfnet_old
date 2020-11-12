import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UpdateMyInformationComponent } from './update-my-information.component';

describe('UpdateMyInformationComponent', () => {
  let component: UpdateMyInformationComponent;
  let fixture: ComponentFixture<UpdateMyInformationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ UpdateMyInformationComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(UpdateMyInformationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
