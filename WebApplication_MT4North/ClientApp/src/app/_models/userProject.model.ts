import { User, Project } from '@app/_models';

export class UserProject {
  userprojectid: string;
  role: string;
  rights: string;
  projectid: string;
  userid: string;
  user: User;
  status: number;
  project: Project;
}
