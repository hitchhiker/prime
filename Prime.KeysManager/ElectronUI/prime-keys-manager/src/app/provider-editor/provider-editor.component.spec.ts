import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProviderEditorComponent } from './provider-editor.component';

describe('ProviderEditorComponent', () => {
  let component: ProviderEditorComponent;
  let fixture: ComponentFixture<ProviderEditorComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProviderEditorComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProviderEditorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
