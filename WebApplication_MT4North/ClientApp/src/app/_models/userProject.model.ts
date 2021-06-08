import { User, Project } from '@app/_models';

export class UserProject {
  userProjectId: number;
  role: string;
  rights: string;
  projectId: number;
  userId: string;
  user: User;
  status: string;
  project: Project;
}
