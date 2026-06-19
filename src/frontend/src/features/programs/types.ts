export interface Program {
  id: string;
  name: string;
  description: string;
  weeks: ProgramWeek[];
}

export interface ProgramWeek {
  name: string;
  sessions: ProgramSession[];
}

export interface ProgramSession {
  name: string;
  exercises: string[];
}
