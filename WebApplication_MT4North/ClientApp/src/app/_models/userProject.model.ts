import { User, Project } from '@app/_models';

export class UserProject {
  userProjectId: string;
  role: string;
  rights: string;
  projectId: string;
  userId: string;
  user: User;
  status: number;
  project: Project;
}
