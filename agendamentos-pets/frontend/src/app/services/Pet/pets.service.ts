import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Pet } from '../../Models/pet';

@Injectable({
  providedIn: 'root'
})
export class PetsService {
  private API = 'http://localhost:5159/api/'; // Completed the URL
  constructor(private http: HttpClient) { }


  getPets(): Observable<Pet[]> {
    return this.http.get<Pet[]>(`${this.API}pet/ListarTodosPets`);
  }

  addPet(pet: Pet): Observable<string> {

    const body = {
      tutor: pet.tutor,
      emailTutor: pet.emailTutor,
      nome: pet.nome,
      cor: pet.cor,
      sexo: pet.sexo,
      idade: pet.idade.toString(),
      especie: pet.tipo,
      nomeRaca: pet.nomeRaca
    };
    
    const headers = new HttpHeaders({ 'Content-type': 'application/json' });
    return this.http.post(`${this.API}pet/CriarPet`, body, { headers, responseType: 'text' });
  }

  deletePet(petId: number): Observable<void> {
    return this.http.delete<void>(`${this.API}pet/DeletarPet?id=${petId}`);
  }

  getPetById(id: string): Observable<Pet> {
    return this.http.get<Pet>(`${this.API}pet/BuscarPetPorId/?id=${id}`);
  }

  updatePet(id: string, petAtualizado: Pet): Observable<any> {
    return this.http.put<any>(`${this.API}pet/AtualizarPet/?id=${id}`, petAtualizado);
  }

  buscarRacas(especie: string): Observable<any> {
    return this.http.get<any>(`${this.API}raca/ListarRacas/?especie=${especie}`);
  }

}
