import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { PetsService } from '../../../../services/Pet/pets.service';
import { ConfirmDialogComponent } from '../../../confirm-dialog/confirm-dialog.component';

// Importando módulos do Angular Material
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatSelectModule } from '@angular/material/select';


@Component({
  selector: 'app-editarpet',
  standalone: true,
  templateUrl: './editarpet.component.html',
  styleUrls: ['./editarpet.component.css'],
  imports: [
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatDialogModule,
    MatSelectModule,
    ReactiveFormsModule
  ]
})
export class EditarpetComponent implements OnInit {
  petForm!: FormGroup;
  petId!: string;
  petSelecionado: any = null;

  private fb = inject(FormBuilder);
  private route = inject(ActivatedRoute);
  private petsService = inject(PetsService);
  private router = inject(Router);

  constructor(private dialog: MatDialog) { }

  ngOnInit(): void {
    // Pegando o ID da URL
    this.petId = this.route.snapshot.paramMap.get('id') || '';

    // 🔥 Inicializa o formulário com valores vazios antes de carregar os dados do pet
    this.petForm = this.fb.group({
      nome: [''],
      nomeRaca: [''],
      idade: [''],
      especie: [''],
      sexo: [''],
      tutor: [''],
      emailTutor: [''],
      cor: ['']
    });

    // Buscando os dados do pet para preencher o formulário
    this.petsService.getPetById(this.petId).subscribe((pet) => {
      if (pet) {
        this.petSelecionado = pet;
        this.petForm.patchValue(pet); // ✅ Atualiza o formulário sem recriá-lo
      }
    });
  }

  atualizarPet(): void {
    if (this.petForm.invalid) {
      this.petForm.markAllAsTouched(); // Isso destaca os erros nos campos
      return;
    }
    if (this.petForm.valid) {
      const dialogRef = this.dialog.open(ConfirmDialogComponent, {
        data: {
          title: 'Editar Pet',
          message: 'Você quer confirmar esta ação?',
          confirmText: 'Sim',
          cancelText: 'Não',
          buttonColor: 'accent'
        }
      });

      dialogRef.afterClosed().subscribe(result => {
        if (result) {
          const petAtualizado = {
            ...this.petSelecionado,
            ...this.petForm.value,
            idade: String(this.petForm.value.idade) // Convertendo idade para string
          };

          console.log('Pet atualizado:', petAtualizado);

          this.petsService.updatePet(this.petId, petAtualizado).subscribe({
            next: () => {
              alert('Pet atualizado com sucesso!');
              this.router.navigate(['/pets']); // Redireciona para a lista de pets
            },
            error: (err) => {
              console.error('Erro ao atualizar pet', err);
              alert('Erro ao atualizar pet.');
            }
          });
        }
      });
    }
  }

  fecharFormulario(): void {
    this.router.navigate(['/pets']); // Voltar para a lista sem salvar
  }
}
