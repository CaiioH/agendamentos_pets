import { Component } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-menu',
  imports: [MatIconModule],
  templateUrl: './menu.component.html',
  styleUrl: './menu.component.css'
})
export class MenuComponent {
  menuAberto = false;

  navigate(url : string) {
    window.location.href = url;
  }

  toggleMenu() {
    this.menuAberto = !this.menuAberto;
  }
}
