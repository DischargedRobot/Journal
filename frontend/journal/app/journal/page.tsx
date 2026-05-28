"use client"
import Journal from "@/_page/journal"
import { Header } from "@/widgets/header"

const JournalPage = () => {
	return (
		<>
			<Header />
			<main className="my-0 mx-auto w-fit overflow-auto">
				<Journal />
			</main>
		</>
	)
}

export default JournalPage
