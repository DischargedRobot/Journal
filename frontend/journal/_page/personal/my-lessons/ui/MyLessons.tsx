"use client"
import { LessonJournalTable } from "@/widgets/lesson-journal-table"
import { mockDisciplines, mockJournalRows, mockLessons } from "@/shared/model/mocks"

const MyLessons = () => {
    return (
        <LessonJournalTable
            lessons={mockLessons}
            rows={mockJournalRows}
            discipline={mockDisciplines[0]}
        />
    )
}

export default MyLessons