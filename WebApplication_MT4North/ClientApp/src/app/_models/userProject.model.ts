import { User, Project, ProjectRole } from '@app/_models';

export class UserProject {
  userprojectid: string;
  role: ProjectRole;
  rights: number;
  projectid: string;
  userid: string;
  user: User;
  status: number;
  project: Project;
  isEditable = false;
}

