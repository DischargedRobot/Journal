import { TJournalRow } from "@/shared/model/lesson"
import { mockLessons } from "./mockLessons"
import { mockJournalStudents } from "./mockStudents"

export const mockJournalRows: TJournalRow[] = [
	{
		student: mockJournalStudents[0],
		order: 1,
		fullName: "Фамилия И. О.",
		lessons: new Map([
			[mockLessons[0].uuid, { presenceStatus: "Н", mark: "неуд." }],
			[mockLessons[1].uuid, { presenceStatus: "Н", mark: "зачтено" }],
			[mockLessons[2].uuid, { presenceStatus: "Н", mark: "зачтено" }],
			[mockLessons[3].uuid, { presenceStatus: "Н", mark: "зачтено" }],
		]),
		percent: 38,
		attendedTotal: "",
		attestation: "Хор",
	},
	{
		student: mockJournalStudents[1],
		order: 5,
		fullName: "Фамилия И. О.",
		lessons: new Map([
			[mockLessons[0].uuid, { presenceStatus: "1/2", mark: "зачтено" }],
			[mockLessons[1].uuid, { presenceStatus: "1/2", mark: "зачтено" }],
			[mockLessons[2].uuid, { presenceStatus: "1/2", mark: "зачтено" }],
			[mockLessons[3].uuid, { presenceStatus: "1/2", mark: "зачтено" }],
		]),
		percent: 38,
		attendedTotal: "",
		attestation: "Хор",
	},
	{
		student: mockJournalStudents[2],
		order: 3,
		fullName: "Фамилия И. О.",
		lessons: new Map([
			[mockLessons[0].uuid, { presenceStatus: "Б", mark: "5" }],
			[mockLessons[1].uuid, { presenceStatus: "Б", mark: "5" }],
			[mockLessons[2].uuid, { presenceStatus: "Б", mark: "5" }],
			[mockLessons[3].uuid, { presenceStatus: "Б", mark: "5" }],
		]),
		percent: 38,
		attendedTotal: "",
		attestation: "Хор",
	},
]
