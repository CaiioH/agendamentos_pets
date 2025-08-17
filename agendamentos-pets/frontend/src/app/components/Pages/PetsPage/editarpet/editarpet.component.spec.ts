import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditarpetComponent } from './editarpet.component';

describe('EditarpetComponent', () => {
  let component: EditarpetComponent;
  let fixture: ComponentFixture<EditarpetComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EditarpetComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EditarpetComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
