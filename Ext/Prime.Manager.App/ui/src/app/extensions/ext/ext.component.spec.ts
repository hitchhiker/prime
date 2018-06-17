import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ExtComponent } from './ext.component';

describe('ExtComponent', () => {
  let component: ExtComponent;
  let fixture: ComponentFixture<ExtComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ExtComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ExtComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
