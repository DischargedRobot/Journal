import { TStudent } from "@/shared/model/student"
import { TPresencesStatus } from "@/shared/model/presences-status"
import { mockJournalGroups, mockPersonalGroups } from "./mockGroups"
import { mockPersonalRoles } from "./mockRoles"
import { mockDisciplines } from "./mockDisciplines"
import { mockLessons } from "./mockLessons"
import { setStudentAttestationMark } from "./mockAttestations"

export const mockJournalStudents: TStudent[] = [
	{
		uuid: "1",
		studentCode: 1,
		firstName: "Иван",
		lastName: "Иванов",
		patronymic: "Иванович",
		groupUuid: "1",
		group: mockJournalGroups[0],
		brigadesUuids: [],
		brigades: [],
		lessons: new Map(),
		roles: [],
		attestations: new Map(),
		version: 1,
	},
	{
		uuid: "2",
		studentCode: 2,
		firstName: "Петр",
		lastName: "Петров",
		patronymic: "Петрович",
		groupUuid: "2",
		group: mockJournalGroups[1],
		brigadesUuids: [],
		brigades: [],
		lessons: new Map(),
		roles: [],
		attestations: new Map(),
		version: 1,
	},
	{
		uuid: "3",
		studentCode: 3,
		firstName: "Сидор",
		lastName: "Сидоров",
		patronymic: "Сидорович",
		groupUuid: "3",
		group: mockJournalGroups[2],
		brigadesUuids: [],
		brigades: [],
		lessons: new Map(),
		roles: [],
		attestations: new Map(),
		version: 1,
	},
]

export const mockPersonalStudents: TStudent[] = [
	{
		uuid: "11111111-1111-1111-1111-111111111101",
		studentCode: 111111,
		firstName: "Кирилл",
		lastName: "Авдеев",
		patronymic: "Владимирович",
		groupUuid: mockPersonalGroups[0].uuid,
		brigadesUuids: [],
		brigades: [],
		roles: mockPersonalRoles,
		version: 1,
		group: mockPersonalGroups[0],
		lessons: new Map(),
		attestations: new Map(),
	},
	{
		uuid: "11111111-1111-1111-1111-111111111102",
		studentCode: 222222,
		firstName: "Анастасия",
		lastName: "Петрова",
		patronymic: "Сергеевна",
		groupUuid: mockPersonalGroups[0].uuid,
		brigadesUuids: [],
		brigades: [],
		version: 1,
		group: mockPersonalGroups[0],
		roles: mockPersonalRoles,
		lessons: new Map(),
		attestations: new Map(),
	},
	{
		uuid: "11111111-1111-1111-1111-111111111103",
		studentCode: 333333,
		firstName: "Дмитрий",
		lastName: "Соколов",
		patronymic: "Александрович",
		groupUuid: mockPersonalGroups[0].uuid,
		brigadesUuids: [],
		brigades: [],
		version: 1,
		group: mockPersonalGroups[0],
		roles: mockPersonalRoles,
		lessons: new Map(),
		attestations: new Map(),
	},
	{
		uuid: "11111111-1111-1111-1111-111111111104",
		studentCode: 444444,
		firstName: "Мария",
		lastName: "Кузнецова",
		patronymic: "Игоревна",
		groupUuid: mockPersonalGroups[0].uuid,
		brigadesUuids: [],
		brigades: [],
		version: 1,
		group: mockPersonalGroups[0],
		roles: mockPersonalRoles,
		lessons: new Map(),
		attestations: new Map(),
	},
	{
		uuid: "11111111-1111-1111-1111-111111111105",
		studentCode: 555555,
		firstName: "Артём",
		lastName: "Волков",
		patronymic: "Николаевич",
		groupUuid: mockPersonalGroups[0].uuid,
		brigadesUuids: [],
		brigades: [],
		version: 1,
		group: mockPersonalGroups[0],
		roles: mockPersonalRoles,
		lessons: new Map(),
		attestations: new Map(),
	},
] satisfies TStudent[]

const mathDiscipline = mockDisciplines[0]

for (const [student, mark] of [
	[mockJournalStudents[0], "4"],
	[mockJournalStudents[1], "5"],
	[mockJournalStudents[2], "5"],
] as const) {
	setStudentAttestationMark(student, mathDiscipline, mark)
}

for (const [student, mark] of [
	[mockPersonalStudents[0], "4"],
	[mockPersonalStudents[1], "5"],
	[mockPersonalStudents[2], "3"],
	[mockPersonalStudents[3], "4"],
	[mockPersonalStudents[4], "5"],
] as const) {
	setStudentAttestationMark(student, mathDiscipline, mark)
}

for (const [student, presenceStatuses] of [
	[mockPersonalStudents[0], ["О", "Н", "О", "Б", "О"]],
	[mockPersonalStudents[1], ["О", "О", "О", "О", "Н"]],
	[mockPersonalStudents[2], ["Н", "О", "О", "О", "О"]],
	[mockPersonalStudents[3], ["О", "О", "Н", "О", "О"]],
	[mockPersonalStudents[4], ["Б", "О", "О", "Н", "О"]],
] as ReadonlyArray<readonly [TStudent, readonly TPresencesStatus[]]>) {
	mockLessons.forEach((lesson, index) => {
		student.lessons.set(lesson.uuid, {
			presenceStatus: presenceStatuses[index],
			mark: "",
		})
	})
}
