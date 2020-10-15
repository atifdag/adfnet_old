import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WaitingSnackbarComponent } from './waiting-snackbar.component';

describe('WaitingSnackbarComponent', () => {
  let component: WaitingSnackbarComponent;
  let fixture: ComponentFixture<WaitingSnackbarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ WaitingSnackbarComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(WaitingSnackbarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
