"use client"
import { LessonJournalTable } from "@/widgets/lesson-journal-table"
import { mockDisciplines, mockJournalRows, mockLessons, mockPersonalGroups, mockPersonalStudents } from "@/shared/model/mocks"
import { Sidebar } from "@/shared/ui/sidebar"
import Box from "@mui/material/Box"
import { StatCard } from "@/shared/ui/stat-card"
import { VisitIcon } from "@/shared/ui/visit-icon"
import { AttestationIcon } from "@/shared/ui/attestation-icon"
import { StudentIcon } from "@/shared/ui/student-icon"
const averageStudentsPerLesson =
    mockLessons.reduce((sum, lesson) => {
        const present = mockPersonalStudents.reduce(
            (acc, student) =>
                acc + ((student.lessons.get(lesson.uuid)?.presenceStatus ?? "Н") === "О" ? 1 : 0),
            0,
        )
        return sum + present
    }, 0) / mockLessons.length

const MyLessons = () => {

    return (
        <div className="flex h-full">
            <Sidebar open={true} onClose={() => { }} title="Группы" items={mockPersonalGroups.map((group, index) => ({
                text: group.name,
                href: `/personal/groups/${group.uuid}`,
                onClick: () => { },
                selected: index === 0,
            }))} />
            <div className="flex flex-col gap-4 px-16 py-8">
                <Box className="flex gap-4">
                    <StatCard
                        icon={<StudentIcon />}
                        value={mockPersonalStudents.length}
                        label="Количество студентов"
                    />
                    <StatCard
                        icon={<VisitIcon />}
                        value={averageStudentsPerLesson}
                        label="Посещаемость"
                    />
                    <StatCard
                        icon={<AttestationIcon />}
                        value={mockPersonalStudents.reduce((acc, student) =>
                            acc + Number(student.attestations.get(mockDisciplines[0].uuid)?.attestationMark?.mark ?? 0) / mockPersonalStudents.length, 0,
                        )}
                        label="Средний балл"
                    />
                </Box>
                <LessonJournalTable
                    lessons={mockLessons}
                    rows={mockJournalRows}
                    discipline={mockDisciplines[0]}
                />
            </div>

        </div>
    )
}

export default MyLessons