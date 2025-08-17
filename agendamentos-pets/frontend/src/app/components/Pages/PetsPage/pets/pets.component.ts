import { Component } from '@angular/core';
import { PetsService } from '../../../../services/Pet/pets.service';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { ConfirmDialogComponent } from '../../../confirm-dialog/confirm-dialog.component';
import { FormsModule } from '@angular/forms';  // ✅ IMPORTAR FormsModule
import { Router } from '@angular/router'; // ✅ Usar Router para navegação
import { Pet } from '../../../../Models/pet';
import { NgxPaginationModule } from 'ngx-pagination';
import { MatIcon } from '@angular/material/icon';



@Component({
  selector: 'app-pets',
  standalone: true,
  templateUrl: './pets.component.html',
  styleUrls: ['./pets.component.css'],
  imports: [MatDialogModule, FormsModule, NgxPaginationModule, MatIcon]
})
export class PetsComponent {
  paginaAtual: number = 1;  // Página atual
  itensPorPagina: number = 9;  // Quantos pets por página
  pets: Pet[] = [];
  petsFiltrados: Pet[] = [];
  filtroSelecionado: string = '';
  valorFiltro: string = '';
  expandedPetId: number | null = null; // Guarda o ID do pet expandido


  constructor(
    private petsService: PetsService,
    private dialog: MatDialog,
    private router: Router // ✅ Substitui `window.location.href`

  ) { }

  ngOnInit(): void {
    this.listPets();
  }

  listPets(): void {
    this.petsService.getPets().subscribe((pets: any[]) => {
      this.pets = pets.map(pet => ({ ...pet, id: String(pet.id) })); // Garante que o ID é string
      this.petsFiltrados = [...this.pets]; //inicia sem filtros
    });
  }

  navigateToCadastro() {
    this.router.navigate(['/cadastro']);
  }

  navigateToEdit(petId: any) {
    this.router.navigate(['/editarpet', petId]);
  }

  expandeDetalhe(petId: number): void {
    // Se o pet já estiver expandido, fecha. Senão, expande.
    this.expandedPetId = this.expandedPetId === petId ? null : petId;
  }

  deletePet(petId: any) {
    const confirmar = this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: 'Remover Pet',
        message: 'Tem certeza que deseja remover este pet?',
        confirmText: 'Confirmar',
        cancelText: 'Cancelar',
        buttonColor: 'warn' // Cor vermelha
      }
    });

    confirmar.afterClosed().subscribe(result => {
      if (result) {
        this.petsService.deletePet(petId).subscribe({
          next: () => {
            this.pets = this.pets.filter(pet => pet.id !== petId);

            //Essa validação impede erros caso this.filtroSelecionado tenha um valor inválido.
            if (!this.filtroSelecionado || !(this.filtroSelecionado in this.pets[0])) {
              this.petsFiltrados = [...this.pets];
              return;
            }
            //Isso informa ao TypeScript que key é uma chave válida da interface Pet, evitando o erro.
            const key = this.filtroSelecionado as keyof Pet;

            this.petsFiltrados = this.pets.filter(pet =>
              !this.filtroSelecionado ||
              pet[key]?.toString().toLowerCase().includes(this.valorFiltro.toLowerCase())
            );
          },
          error: (error) => {
            console.error('Erro ao deletar pet:', error);
          }
        });
      }
    });
  }

  filtrarPets(): void {
    if (!this.filtroSelecionado || !this.valorFiltro.trim()) {
      this.petsFiltrados = [...this.pets];
      return;
    }

    //Isso informa ao TypeScript que key é uma chave válida da interface Pet, evitando o erro.
    const key = this.filtroSelecionado as keyof Pet;

    this.petsFiltrados = this.pets.filter(pet =>
      pet[key]?.toString().toLowerCase().includes(this.valorFiltro.toLowerCase())
    );
    this.paginaAtual = 1; // Reseta para a primeira página ao filtrar

  }

  limparFiltro(): void {
    this.filtroSelecionado = '';
    this.valorFiltro = '';
    this.petsFiltrados = [...this.pets];
  }

}
