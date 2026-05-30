import { Header } from "@/widgets/header"

const PersonalLayout = ({ children }: { children: React.ReactNode }) => {
    return (
        <>
            <Header />
            <main className="my-0 mx-auto w-full overflow-auto">
                {children}
            </main>
        </>
    )
}

export default PersonalLayout