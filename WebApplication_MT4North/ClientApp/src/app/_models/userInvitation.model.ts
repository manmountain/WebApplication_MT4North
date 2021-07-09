import { ProjectRights, ProjectRole } from '@app/_models';

export class UserInvitation {
  id: number;
  email: string;
  role: ProjectRole;
  permissions: ProjectRights;

  constructor(id: number, email: string, role: ProjectRole, permissions: ProjectRights) {
    this.id = id;
    this.email = email;
    this.role = role;
    this.permissions = permissions;
  }
}
