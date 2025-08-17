import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Agenda } from '../../Models/Agenda';

@Injectable({
  providedIn: 'root'
})
export class AgendaService {

  private API = 'http://localhost:5159/api/agendamento'; // Completed the URL
  constructor(private http: HttpClient) { }

  getAgendamentos(): Observable<Agenda[]> {
    return this.http.get<Agenda[]>(`${this.API}/ListarTodosAgendamentos`);
  }

  getAgendamentoById(agendaId: string): Observable<Agenda> {
    return this.http.get<Agenda>(`${this.API}/BuscarAgendamentoPorId/?id=${agendaId}`);
  }

  deleteAgendamento(agendaId: number): Observable<void> {
    return this.http.delete<void>(`${this.API}/DeletarAgendamento?id=${agendaId}`);
  }

  novoAgendamento(agenda: Agenda): Observable<string> {
    const body = {
      servico: agenda.servico,
      descricao: agenda.descricao,
      // petId: agenda.petId,
      // nomePet: agenda.nomePet,
      // tutor: agenda.tutor,
      // emailTutor: agenda.emailTutor,
      dataAgendamento: agenda.dataAgendamento
    };
    return this.http.post(`${this.API}/CriarAgendamentoManual/?nomePet=${agenda.nomePet}&emailTutor=${agenda.emailTutor}`, body, { responseType: 'text' });
  }
  
  updateAgendamento(agendaId: string, agendaAtualizada: Agenda): Observable<any> {
    return this.http.put<any>(`${this.API}/AtualizarAgendamento/?id=${agendaId}`, agendaAtualizada);
  }

}
