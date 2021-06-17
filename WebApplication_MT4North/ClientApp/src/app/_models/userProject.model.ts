import { User, Project } from '@app/_models';

export class UserProject {
  userprojectid: string;
  role: string;
  rights: string;
  projectid: string;
  userId: string;
  user: User;
  status: number;
  project: Project;
}
