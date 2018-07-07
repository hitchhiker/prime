import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ExchangeDialogComponent } from './exchange-dialog.component';

describe('ExchangeDialogComponent', () => {
  let component: ExchangeDialogComponent;
  let fixture: ComponentFixture<ExchangeDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ExchangeDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ExchangeDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
