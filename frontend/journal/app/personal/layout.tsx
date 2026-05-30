import { Header } from "@/widgets/header"
import { BackPanelPersonalMenu } from "@/widgets/back-panel-personal-menu"

const PersonalLayout = ({ children }: { children: React.ReactNode }) => {
    return (
        <>
            <Header />
            <main className="my-0 mx-auto w-full h-full overflow-auto">
                <BackPanelPersonalMenu />
                {children}
            </main>
        </>
    )
}

export default PersonalLayout;